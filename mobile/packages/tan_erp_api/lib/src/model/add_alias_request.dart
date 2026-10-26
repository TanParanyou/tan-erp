//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:tan_erp_api/src/model/localized_text_input.dart';
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'add_alias_request.g.dart';

/// AddAliasRequest
///
/// Properties:
/// * [alias] 
@BuiltValue()
abstract class AddAliasRequest implements Built<AddAliasRequest, AddAliasRequestBuilder> {
  @BuiltValueField(wireName: r'alias')
  LocalizedTextInput get alias;

  AddAliasRequest._();

  factory AddAliasRequest([void updates(AddAliasRequestBuilder b)]) = _$AddAliasRequest;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(AddAliasRequestBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<AddAliasRequest> get serializer => _$AddAliasRequestSerializer();
}

class _$AddAliasRequestSerializer implements PrimitiveSerializer<AddAliasRequest> {
  @override
  final Iterable<Type> types = const [AddAliasRequest, _$AddAliasRequest];

  @override
  final String wireName = r'AddAliasRequest';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    AddAliasRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    yield r'alias';
    yield serializers.serialize(
      object.alias,
      specifiedType: const FullType(LocalizedTextInput),
    );
  }

  @override
  Object serialize(
    Serializers serializers,
    AddAliasRequest object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required AddAliasRequestBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'alias':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType(LocalizedTextInput),
          ) as LocalizedTextInput;
          result.alias.replace(valueDes);
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  AddAliasRequest deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = AddAliasRequestBuilder();
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


