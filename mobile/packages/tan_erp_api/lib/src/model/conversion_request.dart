//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'conversion_request.g.dart';

/// ConversionRequest
///
/// Properties:
/// * [sourceVersion] 
/// * [siteSurveyRevisionId] 
@BuiltValue()
abstract class ConversionRequest implements Built<ConversionRequest, ConversionRequestBuilder> {
  @BuiltValueField(wireName: r'sourceVersion')
  int? get sourceVersion;

  @BuiltValueField(wireName: r'siteSurveyRevisionId')
  String? get siteSurveyRevisionId;

  ConversionRequest._();

  factory ConversionRequest([void updates(ConversionRequestBuilder b)]) = _$ConversionRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(ConversionRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<ConversionRequest> get serializer => _$ConversionRequestSerializer();
}

class _$ConversionRequestSerializer implements PrimitiveSerializer<ConversionRequest> {
  @override
  final Iterable<Type> types = const [ConversionRequest, _$ConversionRequest];

  @override
  final String wireName = r'ConversionRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    ConversionRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.sourceVersion != null) {
      yield r'sourceVersion';
      yield serializers.serialize(
        object.sourceVersion,
        specifiedType: const FullType(int),
      );
    }
    if (object.siteSurveyRevisionId != null) {
      yield r'siteSurveyRevisionId';
      yield serializers.serialize(
        object.siteSurveyRevisionId,
        specifiedType: const FullType(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    ConversionRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required ConversionRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'sourceVersion':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(int),
          ) as int?;
          if (valueDes == null) continue;
          result.sourceVersion = valueDes;
          break;
        case r'siteSurveyRevisionId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.siteSurveyRevisionId = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  ConversionRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = ConversionRequestBuilder();
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


