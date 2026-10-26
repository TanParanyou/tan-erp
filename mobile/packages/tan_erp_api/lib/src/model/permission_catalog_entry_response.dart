//
// AUTO-GENERATED FILE, DO NOT MODIFY!
//

// ignore_for_file: unused_element
import 'package:built_value/built_value.dart';
import 'package:built_value/serializer.dart';

part 'permission_catalog_entry_response.g.dart';

/// PermissionCatalogEntryResponse
///
/// Properties:
/// * [key] 
/// * [description] 
/// * [module] 
/// * [requiresMakerChecker] 
/// * [isDocumentApproval] 
/// * [isApprovalPermission] 
@BuiltValue()
abstract class PermissionCatalogEntryResponse implements Built<PermissionCatalogEntryResponse, PermissionCatalogEntryResponseBuilder> {
  @BuiltValueField(wireName: r'key')
  String? get key;

  @BuiltValueField(wireName: r'description')
  String? get description;

  @BuiltValueField(wireName: r'module')
  String? get module;

  @BuiltValueField(wireName: r'requiresMakerChecker')
  bool? get requiresMakerChecker;

  @BuiltValueField(wireName: r'isDocumentApproval')
  bool? get isDocumentApproval;

  @BuiltValueField(wireName: r'isApprovalPermission')
  bool? get isApprovalPermission;

  PermissionCatalogEntryResponse._();

  factory PermissionCatalogEntryResponse([void updates(PermissionCatalogEntryResponseBuilder b)]) = _$PermissionCatalogEntryResponse;

  @BuiltValueHook(initializeBuilder: true)
  static void _defaults(PermissionCatalogEntryResponseBuilder b) => b;

  @BuiltValueSerializer(custom: true)
  static Serializer<PermissionCatalogEntryResponse> get serializer => _$PermissionCatalogEntryResponseSerializer();
}

class _$PermissionCatalogEntryResponseSerializer implements PrimitiveSerializer<PermissionCatalogEntryResponse> {
  @override
  final Iterable<Type> types = const [PermissionCatalogEntryResponse, _$PermissionCatalogEntryResponse];

  @override
  final String wireName = r'PermissionCatalogEntryResponse';

  Iterable<Object?> _serializeProperties(
    Serializers serializers,
    PermissionCatalogEntryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) sync* {
    if (object.key != null) {
      yield r'key';
      yield serializers.serialize(
        object.key,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.description != null) {
      yield r'description';
      yield serializers.serialize(
        object.description,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.module != null) {
      yield r'module';
      yield serializers.serialize(
        object.module,
        specifiedType: const FullType.nullable(String),
      );
    }
    if (object.requiresMakerChecker != null) {
      yield r'requiresMakerChecker';
      yield serializers.serialize(
        object.requiresMakerChecker,
        specifiedType: const FullType(bool),
      );
    }
    if (object.isDocumentApproval != null) {
      yield r'isDocumentApproval';
      yield serializers.serialize(
        object.isDocumentApproval,
        specifiedType: const FullType(bool),
      );
    }
    if (object.isApprovalPermission != null) {
      yield r'isApprovalPermission';
      yield serializers.serialize(
        object.isApprovalPermission,
        specifiedType: const FullType(bool),
      );
    }
  }

  @override
  Object serialize(
    Serializers serializers,
    PermissionCatalogEntryResponse object, {
    FullType specifiedType = FullType.unspecified,
  }) {
    return _serializeProperties(serializers, object, specifiedType: specifiedType).toList();
  }

  void _deserializeProperties(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
    required List<Object?> serializedList,
    required PermissionCatalogEntryResponseBuilder result,
    required List<Object?> unhandled,
  }) {
    for (var i = 0; i < serializedList.length; i += 2) {
      final key = serializedList[i] as String;
      final value = serializedList[i + 1];
      switch (key) {
        case r'key':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.key = valueDes;
          break;
        case r'description':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.description = valueDes;
          break;
        case r'module':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(String),
          ) as String?;
          if (valueDes == null) continue;
          result.module = valueDes;
          break;
        case r'requiresMakerChecker':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.requiresMakerChecker = valueDes;
          break;
        case r'isDocumentApproval':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isDocumentApproval = valueDes;
          break;
        case r'isApprovalPermission':
          final valueDes = serializers.deserialize(
            value,
            specifiedType: const FullType.nullable(bool),
          ) as bool?;
          if (valueDes == null) continue;
          result.isApprovalPermission = valueDes;
          break;
        default:
          unhandled.add(key);
          unhandled.add(value);
          break;
      }
    }
  }

  @override
  PermissionCatalogEntryResponse deserialize(
    Serializers serializers,
    Object serialized, {
    FullType specifiedType = FullType.unspecified,
  }) {
    final result = PermissionCatalogEntryResponseBuilder();
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


