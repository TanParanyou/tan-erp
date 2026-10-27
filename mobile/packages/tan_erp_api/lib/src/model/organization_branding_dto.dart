//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'organization_branding_dto.g.dart';

/// OrganizationBrandingDto
///
/// Properties:
/// * [primaryColor] 
/// * [logoUrl] 
/// * [logoFileId] 
@BuiltValue()
abstract class OrganizationBrandingDto implements Built<OrganizationBrandingDto, OrganizationBrandingDtoBuilder> {
  @BuiltValueField(wireName: r'primaryColor')
  String? get primaryColor;

  @BuiltValueField(wireName: r'logoUrl')
  String? get logoUrl;

  @BuiltValueField(wireName: r'logoFileId')
  String? get logoFileId;

  OrganizationBrandingDto._();

  factory OrganizationBrandingDto([void updates(OrganizationBrandingDtoBuilder b)]) = _$OrganizationBrandingDto;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(OrganizationBrandingDtoBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<OrganizationBrandingDto> get serializer => _$OrganizationBrandingDtoSerializer();
}

class _$OrganizationBrandingDtoSerializer implements PrimitiveSerializer<OrganizationBrandingDto> {
  @override
  final Iterable<Type> types = const [OrganizationBrandingDto, _$OrganizationBrandingDto];

  @override
  final String wireName = r'OrganizationBrandingDto';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    OrganizationBrandingDto object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.primaryColor != null) {
      yield r'primaryColor';
      yield serializers.serialize(
        object.primaryColor,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.logoUrl != null) {
      yield r'logoUrl';
      yield serializers.serialize(
        object.logoUrl,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.logoFileId != null) {
      yield r'logoFileId';
      yield serializers.serialize(
        object.logoFileId,
        specifiedType: const FullType.nullable(String),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    OrganizationBrandingDto object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required OrganizationBrandingDtoBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'primaryColor':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.primaryColor = valueDes;
          break;
        case r'logoUrl':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.logoUrl = valueDes;
          break;
        case r'logoFileId':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.logoFileId = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  OrganizationBrandingDto deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = OrganizationBrandingDtoBuilder();
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


