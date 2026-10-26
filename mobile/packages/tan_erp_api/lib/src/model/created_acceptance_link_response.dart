//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/acceptance_link_response.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'created_acceptance_link_response.g.dart';

/// CreatedAcceptanceLinkResponse
///
/// Properties:
/// * [link] 
/// * [token] 
/// * [publicPath] 
@BuiltValue()
abstract class CreatedAcceptanceLinkResponse implements Built<CreatedAcceptanceLinkResponse, CreatedAcceptanceLinkResponseBuilder> {
  @BuiltValueField(wireName: r'link')
  AcceptanceLinkResponse? get link;

  @BuiltValueField(wireName: r'token')
  String? get token;

  @BuiltValueField(wireName: r'publicPath')
  String? get publicPath;

  CreatedAcceptanceLinkResponse._();

  factory CreatedAcceptanceLinkResponse([void updates(CreatedAcceptanceLinkResponseBuilder b)]) = _$CreatedAcceptanceLinkResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(CreatedAcceptanceLinkResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<CreatedAcceptanceLinkResponse> get serializer => _$CreatedAcceptanceLinkResponseSerializer();
}

class _$CreatedAcceptanceLinkResponseSerializer implements PrimitiveSerializer<CreatedAcceptanceLinkResponse> {
  @override
  final Iterable<Type> types = const [CreatedAcceptanceLinkResponse, _$CreatedAcceptanceLinkResponse];

  @override
  final String wireName = r'CreatedAcceptanceLinkResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    CreatedAcceptanceLinkResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.link != null) {
      yield r'link';
      yield serializers.serialize(
        object.link,
        specifiedType: const FullType(AcceptanceLinkResponse),
      );
    }
    if (object.token != null) {
      yield r'token';
      yield serializers.serialize(
        object.token,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.publicPath != null) {
      yield r'publicPath';
      yield serializers.serialize(
        object.publicPath,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    CreatedAcceptanceLinkResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required CreatedAcceptanceLinkResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'link':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(AcceptanceLinkResponse),
          ) as AcceptanceLinkResponse?;
          if (valueDes == null) continue;
          result.link.replace(valueDes);
          break;
        case r'token':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.token = valueDes;
          break;
        case r'publicPath':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.publicPath = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  CreatedAcceptanceLinkResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = CreatedAcceptanceLinkResponseBuilder();
    final serializedList = (serialized as Iterable<Object?>).toList();
    final unhandled = <Object?>[];
    _deserializeProperties(
      serializers,
      serialized,
      specifiedType: specifiedType,
      serializedList: serializedList,
      unhandled: unhandled,
      result: result,
    );
    return result.build();
  }
}


